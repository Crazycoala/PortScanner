﻿using System;
using System.Net.Sockets;
using System.Threading.Tasks;


class Program
{
    static void Main(string[] args)
    {
        Console.Write("Введите IP-адрес (например, 127.0.0.1): ");
        string ip = Console.ReadLine();

        Console.Write("Введите начальный порт: ");
        int startPort = int.Parse(Console.ReadLine());

        Console.Write("Введите конечный порт: ");
        int endPort = int.Parse(Console.ReadLine());

        Console.WriteLine($"Сканирование {ip} с порта {startPort} до {endPort}....");
        ScanPorts(ip, startPort, endPort).Wait(); // Ждем завершения сканирования
        Console.WriteLine("Сканирование завершено");

    }

    static async Task ScanPorts(string ip, int startPort, int endPort)
    {
        // Создаем массив задач для паралельного сканирования
        Task[] tasks = new Task[endPort - startPort + 1];
        int taskIndex = 0;

        for(int port = startPort; port <= endPort; port++)
        {
            int currentPort = port; // Локальная переменная для замыкания
            tasks[taskIndex] = Task.Run(() => ScanPort(ip, currentPort));
            taskIndex++;
        }

        // Ждем завершения всех задач
        await Task.WhenAll(tasks);
    }


    static void ScanPort(string ip, int port)
    {
        using(TcpClient client = new TcpClient())
        {
            client.ReceiveTimeout = 1000; // Таймаут 1 секудна
            try
            {
                // Пытаемся подключиться к порту 
                client.Connect(ip, port);
                Console.WriteLine($"Порт {port} открыт");
            }
            catch (SocketException)
            {
                // Порт закрыт - молчим
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подключении порта {port}: {ex.Message}");
            }
        }
    }


}