using System;
using System.Collections.Generic;

namespace Lab3_NhatNguyen_WebApp.Models;

public partial class User
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Displayname { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string? Phonenumber { get; set; }

    public string? Address { get; set; }

    public int Userid { get; set; }
}
