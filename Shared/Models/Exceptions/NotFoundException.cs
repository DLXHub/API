using System;

namespace API.Shared.Models.Exceptions;

public class NotFoundException : Exception
{
  public NotFoundException(string message) : base(message)
  {
  }
}