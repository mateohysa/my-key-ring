using System;
using System.IO;

namespace password_manager_project.config
{
    public static class EnvironmentConfig
    {
        private static readonly Dictionary<string, string> _variables = new();

        static EnvironmentConfig()
        {
            LoadEnvironmentVariables();
        }

        private static void LoadEnvironmentVariables()
        {
            try
            {
                string envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
                if (File.Exists(envPath))
                {
                    foreach (string line in File.ReadAllLines(envPath))
                    {
                        if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                        {
                            var parts = line.Split('=', 2);
                            if (parts.Length == 2)
                            {
                                _variables[parts[0].Trim()] = parts[1].Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading .env file: {ex.Message}");
            }
        }

        public static string GetVariable(string name)
        {
            return _variables.TryGetValue(name, out string? value) 
                ? value 
                : throw new KeyNotFoundException($"Environment variable {name} not found");
        }
    }
} 