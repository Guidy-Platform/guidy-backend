using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace CoursePlatform.Application.Features.Admin.Commands.CreateUserByAdmin
{
   
    public class CreateUserByAdminCommandHandler
        : IRequestHandler<CreateUserByAdminCommand, Guid>
    {
        private readonly UserManager<AppUser> _userManager;

        public CreateUserByAdminCommandHandler(
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Guid> Handle(
            CreateUserByAdminCommand request,
            CancellationToken ct)
        {
            // check email
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new BadRequestException("Email already exists");

            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true // admin created
            };

            // create with password
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            // assign role
            await _userManager.AddToRoleAsync(user, request.Role);

            return user.Id;
        }
    }
}
