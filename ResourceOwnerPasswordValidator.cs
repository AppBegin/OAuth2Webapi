using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using CryptoHelper;

namespace auth
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //repository to get user from db
        private readonly IApplicationUserRepository _userRepository;

        public ResourceOwnerPasswordValidator(IApplicationUserRepository userRepository)
        {
            _userRepository = userRepository; //DI
        }

        //this is used to validate your user account with provided grant at /connect/token
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                await Task.Yield();
                //get your user model from db (by username - in my case its phone)
                var user = _userRepository.FindByPhone(context.UserName);
                if (user != null)
                {
                    if ( ! user.ifValidate){
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "PhoneNotValidate");
                        return;
                    }
                    //check if password match 
                    if  (Crypto.VerifyHashedPassword(user.password,context.Password)) {
                        //set the result
                        context.Result = new GrantValidationResult(
                            subject: user.id.ToString(),
                            authenticationMethod: "custom"); 
                            //claims: GetUserClaims(user));
                        return;
                    } 

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password");
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist.");
                return;
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ex.ToString());
            }
        }

    }
}