using MES.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MES.Infrastructure.Configurations
{
    public class UserGroupRightsConfiguration : IEntityTypeConfiguration<UserGroupRight>
    {
        public void Configure(EntityTypeBuilder<UserGroupRight> builder)
        {
            builder.ToTable("UserGroupRights");

            builder.HasKey(u => u.Id);
        }
    }
}
