namespace TaskCat.Lib.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using MongoDB.Driver;
    using Data.Entity.Identity;
    using Data.Model;
    using Data.Model.Identity;
    using Data.Model.Identity.Profile;
    using Data.Model.Identity.Registration;
    using Data.Model.Identity.Response;
    using Email;
    using Model.Identity;
    using Model.Pagination;
    using Model.Storage;

    public interface IAccountContext
    {
        Task<bool> AddRefreshToken(RefreshToken token);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string code);
        Task<List<User>> FindAll(int page, int pageSize);
        Task<IQueryable<UserModel>> FindAllAsModel();
        Task<List<UserModel>> FindAllAsModel(int page, int pageSize);
        Task<PageEnvelope<UserModel>> FindAllEnvelopedAsModel(int page, int pageSize, HttpRequestMessage request);
        Task<PageEnvelope<Data.Entity.Job>> FindAssignedJobs(string userId, int page, int pageSize, DateTime? dateTimeUpto, JobState jobStateToFetchUpTo, SortDirection dateTimeSortDirection, HttpRequestMessage request);
        Task<PageEnvelope<Data.Entity.Job>> FindAssignedJobsByUserName(string userName, int page, int pageSize, DateTime? dateTimeUpto, JobState jobStateToFetchUpTo, SortDirection dateTimeSortDirection, HttpRequestMessage request);
        Task<Client> FindClient(string clientId);
        Task<RefreshToken> FindRefreshToken(string refreshTokenId);
        Task<User> FindUser(string userId);
        Task<User> FindUser(string userName, string password);
        Task<UserModel> FindUserAsModel(string userId);
        Task<User> FindUserByEmail(string email, string password);
        Task<User> FindUserByUserNameEmailPhoneNumber(string userKey, string password);
        Task<List<RefreshToken>> GetAllRefreshTokens();
        Task<bool> IsEmailAvailable(string email);
        Task<bool> IsPhoneNumberAvailable(string phoneNumber);
        Task<bool> IsUsernameAvailable(string suggestedUsername);
        Task<SendEmailResponse> NotifyUserCreationByMail(User user, HttpRequestMessage message);
        Task<AccountResult> RegisterUser(RegistrationModelBase model);
        Task<bool> RemoveRefreshToken(string hashedTokenId);
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<IdentityResult> Update(IdentityProfile profile, string userName);
        Task<IdentityResult> UpdateById(IdentityProfile model, string userId);
        Task<IdentityResult> UpdateContacts(ContactUpdateModel model, string userName);
        Task<IdentityResult> UpdatePassword(PasswordUpdateModel model, string userName);
        Task<IdentityResult> UpdateUsername(string newUserName, string oldUserName);
        Task<IdentityResult> UpdateUsernameById(string userId, string newUserName);
        Task<FileUploadModel> UploadAvatar(HttpContent content, string userId);
    }
}