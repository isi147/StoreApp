﻿using Aplication.CQRS.Auth.Command.Request;
using Aplication.CQRS.Auth.Command.Response;
using Common.Exceptions;
using Common.GlobalExceptionsResponses.Generics;
using Domain.Entity;
using Domain.Extensions;
using Domain.Helper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Repository.Common;
using System.Net;
using System.Net.Mail;

namespace Aplication.CQRS.Auth.Handler.CommandHandler;

public class RegistrationUserCommandHandler : IRequestHandler<RegistrationUserCommandRequest, ResponseModel<RegistrationUserCommandResponse>>
{
	private readonly IUnitOfWork _unitOfWork;



	public RegistrationUserCommandHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<ResponseModel<RegistrationUserCommandResponse>> Handle(RegistrationUserCommandRequest request, CancellationToken cancellationToken)
	{
		var otp = await _unitOfWork.SentEmailRepository.GetAsync(request.OtpCode);
		if (otp == null)
		{
			throw new BadRequestException();
		}
		var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email);
		if (user != null)
		{
			throw new BadImageFormatException();
		}
		var hashedPassword = PasswordHasher.ComputeStringToSha256Hash(request.Password);
		var newUser = new User()
		{
			Name = request.Name,
			Surname = request.Surname,
			Email = request.Email,
			UserType = request.UserType,
			PasswordHash = hashedPassword,
			CreatedDate = DateTime.Now,
		};


		await _unitOfWork.UserRepository.RegisterAsync(newUser);
		otp.IsUsed = true;
		await _unitOfWork.SaveChangesAsync();
		var response = new RegistrationUserCommandResponse()
		{
			Id = newUser.Id,
			Name = newUser.Name,
			Surname = newUser.Surname,
			Email = newUser.Email,
			UserType = newUser.UserType,
		};
		return new ResponseModel<RegistrationUserCommandResponse>(response);
	}
}
