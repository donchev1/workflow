using Organiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Services
{
    public class AccountService : IAccountService
    {
        IUserRepository _userRepository { get; set; }
        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetUserByNameAndPassword(string userName, string password)
        {
            return _userRepository.GetUserByNameAndPassword(userName, password);

        }

        public IEnumerable<int> GetUserRolesByUserId(int id)
        {
            return _userRepository.GetUserRolesByUserId(id);
        }

    }
}
