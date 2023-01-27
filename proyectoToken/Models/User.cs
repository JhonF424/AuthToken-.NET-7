using System;
using System.Collections.Generic;

namespace proyectoToken.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? Pass { get; set; }
}
