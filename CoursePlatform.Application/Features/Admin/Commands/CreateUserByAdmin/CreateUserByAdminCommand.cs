using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursePlatform.Application.Features.Admin.Commands.CreateUserByAdmin
{
    public record CreateUserByAdminCommand(
      string Email,
      string FirstName,
      string LastName,
      string Role,
      string Password
  ) : IRequest<Guid>;
}
