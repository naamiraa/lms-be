namespace be.Services.Admin;

public interface IAdminService
{
    Task AssignRoleAsync(Guid userId, string roleName);
}