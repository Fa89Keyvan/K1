// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
//
// namespace K1.JwtSample.Data
// {
//     public class ApiDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApiDbContext>
//     {
//         public ApiDbContext CreateDbContext(string[] args)
//         {
//             var optionBuilder = new DbContextOptionsBuilder<ApiDbContext>();
//             optionBuilder.UseSqlServer("Persist Security Info=True;Initial Catalog=K1_JwtSample;Data Source=.");
//             return new ApiDbContext(optionBuilder.Options);
//         }
//     }
// }