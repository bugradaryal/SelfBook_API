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
                new IdentityRole { Name = Authorization.Roles.Administrator.ToString(), NormalizedName = Authorization.Roles.Administrator.ToString().ToUpper() },
                new IdentityRole { Name = Authorization.Roles.Moderator.ToString(), NormalizedName = Authorization.Roles.Moderator.ToString().ToUpper() },
                new IdentityRole { Name = Authorization.Roles.User.ToString(), NormalizedName = Authorization.Roles.User.ToString().ToUpper() }
            );
        }
    }
}
