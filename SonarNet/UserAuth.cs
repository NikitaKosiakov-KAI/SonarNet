using System;

namespace SonarNet
{
    public class UserAuth
    {
        public bool Authenticate(string username, string password)
        {
            // Hotfix: Додано перевірку на порожні значення, щоб уникнути помилок системи
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Критична помилка: Логін або пароль не можуть бути порожніми!");
                return false;
            }

            if (username == "admin" && password == "password123")
            {
                Console.WriteLine("Аутентифікація успішна!");
                return true;
            }
            
            Console.WriteLine("Помилка: Неправильний логін або пароль.");
            return false;
        }
    }
}