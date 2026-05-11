using System;

namespace SonarNet
{
    public class UserAuth
    {
        // Метод для базової перевірки користувача
        public bool Authenticate(string username, string password)
        {
            // Проста заглушка для лабораторної роботи
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