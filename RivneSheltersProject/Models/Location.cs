namespace RivneSheltersProject.Models;

public class Location
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;        // Розташування
    public string Ownership { get; set; } = string.Empty;      // Форма власності
    public string Holder { get; set; } = string.Empty;         // Балансоутримувач
    public string Accessibility { get; set; } = string.Empty;  // Доступність для інвалідів
    public string Contact { get; set; } = string.Empty;        // Контакт відповідального
    public string Number { get; set; } = string.Empty;         // Номер укриття
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
