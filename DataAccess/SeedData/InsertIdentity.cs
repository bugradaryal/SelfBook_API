using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SeedData
{
    public class InsertIdentity : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> entity)
        {
            entity.HasData(
                new IdentityRole { Name = "Administrator", NormalizedName = "Administrator".ToUpper() },
                new IdentityRole { Name = "Moderator", NormalizedName = "Moderator".ToUpper() },
                new IdentityRole { Name = "User", NormalizedName = "User".ToUpper() }
            );
        }
    }
}
