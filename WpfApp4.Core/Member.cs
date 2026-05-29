using System;

namespace WpfApp4.Core;

public class Member
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateTime RegisterDate { get; set; }
}
