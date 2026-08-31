using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace be.Services.Certificate;

public class CertificatePdfService : ICertificatePdfService
{
    public CertificatePdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateCertificatePdf(
        string certificateNumber,
        string participantName,
        string learningModuleName,
        string scenarioName,
        double overallScore,
        DateTime issuedAt)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());

                page.Margin(40);

                page.DefaultTextStyle(
                    x => x.FontSize(14));

                page.Content()
                    .Border(2)
                    .Padding(35)
                    .Column(column =>
                    {
                        column.Spacing(18);

                        column.Item()
                            .AlignCenter()
                            .Text("CERTIFICATE OF COMPLETION")
                            .Bold()
                            .FontSize(30);

                        column.Item()
                            .AlignCenter()
                            .Text("Fleet Safety Training")
                            .FontSize(20);

                        column.Item()
                            .PaddingTop(20)
                            .AlignCenter()
                            .Text("This certificate is awarded to")
                            .FontSize(14);

                        column.Item()
                            .AlignCenter()
                            .Text(participantName)
                            .Bold()
                            .FontSize(28);

                        column.Item()
                            .AlignCenter()
                            .Text("for successfully completing")
                            .FontSize(14);

                        column.Item()
                            .AlignCenter()
                            .Text(learningModuleName)
                            .Bold()
                            .FontSize(22);

                        column.Item()
                            .AlignCenter()
                            .Text($"Simulation Scenario: {scenarioName}");

                        column.Item()
                            .AlignCenter()
                            .Text($"Overall Score: {overallScore:0.##}");

                        column.Item()
                            .PaddingTop(20)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Column(left =>
                                    {
                                        left.Item()
                                            .Text("Certificate Number")
                                            .Bold();

                                        left.Item()
                                            .Text(certificateNumber);
                                    });

                                row.RelativeItem()
                                    .AlignRight()
                                    .Column(right =>
                                    {
                                        right.Item()
                                            .AlignRight()
                                            .Text("Issued Date")
                                            .Bold();

                                        right.Item()
                                            .AlignRight()
                                            .Text(
                                                issuedAt.ToString(
                                                    "dd MMMM yyyy"));
                                    });
                            });

                        column.Item()
                            .PaddingTop(30)
                            .AlignCenter()
                            .Text(
                                "Learning Management System - Fleet Safety Simulator")
                            .FontSize(11);
                    });
            });
        });

        return document.GeneratePdf();
    }
}
