using System;
using System.Globalization;

[Serializable]
public class ReminderData
{
    public string title;
    public string content;
    public string startDateTime; // "dd.MM.yyyy HH:mm"
    public int dayInterval;
    public int notificationId;

    public DateTime GetTargetDate()
    {
        string[] formats = { "dd.MM.yyyy HH:mm", "dd.MM.yyyy HH.mm" };

        if (DateTime.TryParseExact(startDateTime, formats, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out DateTime parsed))
        {
            return parsed;
        }

        throw new FormatException(
            $"Tarih formatı hatalı! '{startDateTime}' | Beklenen: dd.MM.yyyy HH:mm veya dd.MM.yyyy HH.mm"
        );
    }
}