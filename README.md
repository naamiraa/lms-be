# Backend — Fleet Safety LMS

Backend Fleet Safety LMS menyediakan REST API untuk akun pengguna, materi pembelajaran, assessment, sesi simulator, evaluasi, feedback instructor, riwayat pelatihan, dan sertifikat.

Frontend berada di folder [`../fe`](../fe/README.md). Simulator berjalan sebagai aplikasi terpisah dan mengirimkan hasil latihan ke backend melalui API.

## Tech Stack

| Teknologi | Versi | Kegunaan |
| --- | --- | --- |
| C# dan ASP.NET Core Web API | .NET 10 | API berbasis controller |
| Entity Framework Core Design | 10.0.11 | Pengelolaan model dan migration |
| Npgsql EF Core Provider | 10.0.3 | Koneksi EF Core ke PostgreSQL |
| PostgreSQL | 17 | Database |
| JWT Bearer | 10.0.11 | Autentikasi dan akses berdasarkan role |
| ASP.NET Core Identity PasswordHasher | Bawaan framework | Hash dan verifikasi password |
| ASP.NET Core OpenAPI | 10.0.10 | Dokumentasi API |
| Swashbuckle | 10.2.3 | Swagger UI |
| QuestPDF | 2026.8.0 | Pembuatan sertifikat PDF |
| Docker Compose | — | Menjalankan PostgreSQL secara lokal |

## Struktur Project

| Folder / File | Isi |
| --- | --- |
| `Controllers` | Endpoint API, pemeriksaan akses, dan response HTTP |
| `DTOs` | Bentuk data request dan response |
| `Services` | Logika bisnis dan pemrosesan data |
| `Models` | Entity dan hubungan data yang disimpan |
| `Data` | `AppDbContext` dan seeder |
| `Migrations` | Riwayat perubahan struktur database |
| `Settings` | Pengaturan evaluasi melalui `EvaluationSettings` |
| `Properties` | Profil menjalankan aplikasi dan port lokal |
| `Program.cs` | Konfigurasi database, service, JWT, CORS, middleware, dan seeding |
| `appsettings.json` | Pengaturan aplikasi |
| `be.csproj` | Target framework dan dependency |

Alur request:

**Client → Controller → Service → AppDbContext → PostgreSQL**

Controller menerima request dan memeriksa akses. Service menjalankan aturan bisnis serta membaca atau menyimpan data melalui `AppDbContext`. Hasilnya dikembalikan kepada client sebagai response DTO.

Service menggunakan `AppDbContext` secara langsung. Project ini belum memakai repository layer terpisah.

## Role Pengguna

| Role | Akses utama |
| --- | --- |
| `PARTICIPANT` | Belajar, mengerjakan assessment, membuat sesi simulator, dan melihat hasil |
| `INSTRUCTOR` | Meninjau sesi peserta dan memberikan feedback |
| `ADMIN` | Mengelola pengguna, materi, assessment, domain, dan skenario simulator |

Satu pengguna dapat memiliki lebih dari satu role.

Request yang membutuhkan login menggunakan header:

```http
Authorization: Bearer <token>
```

Backend memvalidasi token dan role. Untuk data pribadi, service juga memeriksa apakah data tersebut milik pengguna yang melakukan request.

## Hubungan Data

| Bagian | Hubungan |
| --- | --- |
| Pengguna | `User` memiliki role melalui `UserRole` |
| Learning | `TrainingDomain` → `LearningModule` → `LearningContent` |
| Progress | Penyelesaian materi per pengguna |
| Assessment | Terhubung ke modul dan memiliki soal serta pilihan jawaban |
| Attempt | Percobaan assessment dan jawaban peserta |
| Simulation | `SimulationDomain` → `SimulationScenario` |
| Session | Menghubungkan peserta, assessment attempt, dan skenario |
| Hasil | Skor simulator dan event pelanggaran |
| Feedback | Catatan instructor untuk suatu sesi |
| Certificate | Sertifikat peserta berdasarkan sesi latihan |

Training Domain dan Simulation Domain merupakan data yang berbeda. Training Domain mengelompokkan materi belajar, sedangkan Simulation Domain mengelompokkan skenario simulator.

## Alur Pelatihan

### 1. Registrasi dan Login

Pengguna mendaftar dengan email dan password. Password disimpan dalam bentuk hash. Akun baru mendapat role `PARTICIPANT` dan status `PENDING`.

Admin menyetujui atau menolak registrasi. Login hanya dapat dilakukan oleh akun yang aktif, berstatus `APPROVED`, dan menggunakan password yang benar.

Setelah login, backend mengembalikan JWT beserta identitas dan role pengguna. Masa berlaku token mengikuti `Jwt:ExpiryMinutes`, yang saat ini diatur 120 menit.

### 2. Learning dan Progress

Admin menambahkan domain, modul, dan materi. Peserta membaca materi, lalu menandainya sebagai selesai.

Progress dihitung dari jumlah materi aktif yang selesai dibandingkan seluruh materi aktif dalam modul. Menandai materi yang sudah selesai tidak membuat catatan duplikat.

Modul tanpa materi memiliki progress 0% dan belum dianggap selesai.

### 3. Assessment

Admin menambahkan assessment untuk suatu modul, kemudian mengisi soal, pilihan jawaban, durasi, dan batas attempt.

Saat peserta memulai tes, backend memeriksa assessment, modul, dan soal yang aktif. Attempt `IN_PROGRESS` yang belum kedaluwarsa digunakan kembali. Attempt yang melewati batas waktu ditandai `EXPIRED` saat pemeriksaan terkait dilakukan.

Setiap jawaban disimpan setelah backend memeriksa pemilik attempt, waktu pengerjaan, serta kecocokan soal dan pilihan jawaban.

Submit hanya dapat dilakukan jika seluruh soal aktif sudah dijawab dan waktu belum habis. Skor teori dihitung dengan:

```text
Skor teori = (jumlah jawaban benar / jumlah soal aktif) × 100
```

Hasil dibulatkan dua desimal dan attempt berubah menjadi `COMPLETED`.

Pada implementasi saat ini:

- Memulai assessment belum mensyaratkan progress learning 100%.
- Pembuatan sesi simulator membutuhkan assessment yang selesai, tetapi belum memeriksa kelulusan nilai teori.

### 4. Simulation

Peserta memilih skenario dan mengirimkan:

- `AssessmentAttemptId`
- `SimulationScenarioId`

Backend memastikan attempt tersebut milik peserta dan sudah `COMPLETED`, lalu membuat sesi berstatus `READY`.

Sesi yang belum `COMPLETED` atau `FAILED` dengan peserta, attempt, dan skenario yang sama akan digunakan kembali.

Simulator menggunakan endpoint berikut:

| Endpoint | Fungsi |
| --- | --- |
| `POST /api/simulator/sessions/{sessionId}/start` | Memulai latihan dan mengubah status menjadi `IN_PROGRESS` |
| `POST /api/simulator/sessions/{sessionId}/complete` | Menyimpan hasil latihan dan mengubah status menjadi `COMPLETED` |

Endpoint integrasi saat ini membutuhkan role `ADMIN`.

Body completion berisi skor simulator, skor tiap aspek, durasi, dan daftar pelanggaran. Pelanggaran dikirim melalui array `events`, bukan endpoint violation terpisah.

Untuk pengujian, request dapat dikirim melalui Swagger. Integrasi langsung dilakukan dengan menambahkan pemanggilan API tersebut pada aplikasi simulator.

### 5. Evaluasi dan Feedback

Backend menggabungkan skor teori dan simulator menggunakan `EvaluationCalculator`.

Pengaturan saat ini:

```json
{
  "Evaluation": {
    "TheoryWeight": 0.5,
    "SimulatorWeight": 0.5,
    "PassingScore": 80
  }
}
```

Perhitungannya:

```text
Nilai akhir = round(
    skor teori × TheoryWeight +
    skor simulator × SimulatorWeight,
    2
)

PASS jika nilai akhir ≥ PassingScore
FAIL jika nilai akhir < PassingScore
```

Jumlah bobot harus 1.0. Contohnya, skor teori 90 dan simulator 70 menghasilkan nilai akhir 80, sehingga hasilnya `PASS`.

`Evaluation:PassingScore` digunakan untuk hasil akhir gabungan. Nilai ini berbeda dari `PassingScore` pada masing-masing assessment.

Instructor dapat meninjau hasil sesi dan memberikan komentar, penjelasan pelanggaran, serta rekomendasi. Peserta dapat membaca hasil, feedback, dan riwayat pelatihannya.

### 6. Sertifikat

Sertifikat diterbitkan melalui request untuk sesi yang:

- Sudah selesai.
- Memiliki hasil simulator dan skor teori.
- Mendapat hasil akhir `PASS`.

Jika sertifikat sesi tersebut sudah ada, backend mengembalikan sertifikat yang sama.

`CertificateService` menangani data dan syarat penerbitan. `CertificatePdfService` menggunakan QuestPDF untuk membuat dokumen PDF saat diunduh.

Nilai akhir pada response sertifikat dihitung kembali menggunakan konfigurasi evaluasi yang berlaku. Perubahan bobot atau ambang kelulusan dapat memengaruhi nilai dan hasil yang ditampilkan.

## Endpoint Utama

Detail parameter dan body request tersedia di Swagger.

| Prefix | Kegunaan |
| --- | --- |
| `/api/Auth` | Registrasi dan login |
| `/api/admin/users` | Pengguna, approval, dan role |
| `/api/training-domains` | Domain pembelajaran |
| `/api/learning-modules` | Modul pembelajaran |
| `/api/learning-contents` | Materi |
| `/api/learning-progress` | Progress peserta |
| `/api/assessments` | Assessment |
| `/api/assessment-questions` | Soal dan pilihan jawaban |
| `/api/participant-assessments` | Attempt, jawaban, dan submit |
| `/api/simulation-domains` | Domain simulator |
| `/api/simulation-scenarios` | Skenario simulator |
| `/api/simulation-sessions` | Pembuatan dan pembacaan sesi |
| `/api/simulator` | Start dan completion dari simulator |
| `/api/performance-evaluations` | Hasil evaluasi |
| `/api/instructor-feedback` | Feedback instructor |
| `/api/instructor/sessions` | Daftar sesi untuk instructor |
| `/api/training-history` | Riwayat pelatihan |
| `/api/certificates` | Sertifikat dan download PDF |
| `/api/dashboard` | Dashboard berdasarkan role |

## Menjalankan Project

Prasyarat:

- .NET SDK 10.
- Docker dengan Docker Compose.
- Tool `dotnet-ef` versi mayor 10.

### 1. Jalankan Database

Dari root repository:

```powershell
docker compose up -d postgres
```

Compose menjalankan PostgreSQL saja. Frontend dan backend dijalankan terpisah.

Database dapat diakses dari laptop melalui:

```text
Host: localhost
Port: 35001
Database: lms_db
```

Username dan password mengikuti konfigurasi lokal pada `docker-compose.yml`.

### 2. Atur Konfigurasi Backend

Masuk ke folder backend:

```powershell
cd be
```

Simpan connection string dan signing key lokal melalui user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=35001;Database=lms_db;Username=<user-compose>;Password=<password-compose>"

dotnet user-secrets set "Jwt:Key" "<signing-key-lokal-minimal-32-byte>"
```

Ganti placeholder dengan nilai sebenarnya. Jika project belum memiliki `UserSecretsId`, jalankan:

```powershell
dotnet user-secrets init
```

Pengaturan yang digunakan:

| Key | Kegunaan |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | Koneksi PostgreSQL |
| `Jwt:Key` | Signing key token |
| `Jwt:Issuer` | Penerbit token; saat ini `LmsApi` |
| `Jwt:Audience` | Penerima token; saat ini `LmsClient` |
| `Jwt:ExpiryMinutes` | Masa berlaku token |
| `Evaluation:TheoryWeight` | Bobot skor teori |
| `Evaluation:SimulatorWeight` | Bobot skor simulator |
| `Evaluation:PassingScore` | Ambang kelulusan akhir |
| `BootstrapAdmin:Email` | Email akun admin awal |
| `BootstrapAdmin:Password` | Password akun admin awal |
| `BootstrapAdmin:Name` | Nama akun admin awal |

Untuk menyiapkan akun admin awal:

```powershell
dotnet user-secrets set "BootstrapAdmin:Email" "admin@example.test"
dotnet user-secrets set "BootstrapAdmin:Password" "<password-admin-lokal>"
dotnet user-secrets set "BootstrapAdmin:Name" "LMS Administrator"
```

Seeder membuat akun saat startup jika email belum terdaftar. Jika email atau password bootstrap tidak diisi, pembuatan akun awal dilewati.

### 3. Terapkan Migration dan Jalankan API

```powershell
dotnet restore
dotnet tool install --global dotnet-ef --version "10.*"
dotnet ef database update
dotnet run --launch-profile http
```

Lewati instalasi `dotnet-ef` jika versi yang sesuai sudah tersedia.

Migration tidak dijalankan otomatis saat startup. Jalankan `dotnet ef database update` terlebih dahulu agar tabel tersedia sebelum seeder berjalan.

Alamat lokal:

| Layanan | URL |
| --- | --- |
| API HTTP | http://localhost:5216 |
| Swagger | http://localhost:5216/swagger |
| OpenAPI | http://localhost:5216/openapi/v1.json |
| API HTTPS | https://localhost:7041 |

CORS saat ini mengizinkan frontend pada `http://localhost:3000`. Jika alamat frontend berubah, sesuaikan policy `Frontend` di `Program.cs`.

Untuk menjalankan profil HTTPS:

```powershell
dotnet dev-certs https --trust
dotnet run --launch-profile https
```

Sesuaikan alamat API pada frontend dengan profil backend yang dijalankan.