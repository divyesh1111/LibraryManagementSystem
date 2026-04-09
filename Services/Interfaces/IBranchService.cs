using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<LibraryBranch>> GetAllBranchesAsync();
        Task<LibraryBranch?> GetBranchByIdAsync(int id);
        Task<LibraryBranch?> GetBranchWithDetailsAsync(int id);
        Task<LibraryBranch> CreateBranchAsync(LibraryBranch branch);
        Task<LibraryBranch?> UpdateBranchAsync(int id, LibraryBranch branch);
        Task<bool> DeleteBranchAsync(int id);
        Task<bool> BranchHasBooksOrLoansAsync(int id);
    }
}