using System;
using System.Collections.Generic;

namespace proyectoToken.Models;

public partial class HistoryRefreshToken
{
    public int IdHistoryToken { get; set; }

    public int? UserId { get; set; }

    public string Token { get; set; }

    public string RefreshToken { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual User User { get; set; }
}
