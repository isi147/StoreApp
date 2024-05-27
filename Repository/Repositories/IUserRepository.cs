﻿using Domain.Entity;

namespace Repository.Repositories;

public interface IUserRepository
{
	Task RegisterAsync(User user);
	void Update(User user);
	void Delete(int id);
	IQueryable<User> GetAll();
	Task<User> GetByIdAsync(int id);
	Task<User> GetUserByEmailAsync(string email);


}