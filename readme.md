# 🔐 Password Manager

A secure, desktop-based password manager built with C# and Windows Forms. This application allows users to safely store, manage, and organize their passwords with strong encryption and a clean, intuitive interface.

## ✨ Features

- **Secure Storage**: All passwords are encrypted using industry-standard encryption
- **User Authentication**: Secure login system with hashed master passwords
- **Password Management**:
  - Add, edit, and delete password entries
  - Organize passwords by category
  - Search functionality for quick access
  - Copy passwords to clipboard
  - Password visibility toggle
- **Password Suggestions**:
  - Track password age
  - Suggest passwords that need updating (90+ days old)
  - Built-in password generator for strong passwords
- **Clean UI**:
  - Modern, intuitive interface
  - Easy-to-use sidebar navigation
  - Password strength indicators
  - Clear password organization

## 🛠️ Technical Details

- **Platform**: Windows Desktop
- **Framework**: .NET 6.0
- **UI Framework**: Windows Forms
- **Database**: SQLite
- **Security Features**:
  - AES encryption for stored passwords
  - PBKDF2 with SHA256 for master password hashing
  - Secure random password generation

## 🚀 Getting Started

1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Build and run the application
4. Create a new account with a strong master password
5. Start managing your passwords securely!

## 🔒 Security Notes

- Master passwords are hashed, never stored in plain text
- Individual passwords are encrypted before storage
- No passwords are stored in memory longer than necessary
- Automatic clipboard clearing after password copying
- Session management for security

## 📝 License

[MIT License](LICENSE)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 🙏 Acknowledgments

- UI inspiration from modern password managers and Apple Keychain
- Security best practices from AES encryption.

---
Built by Mateo Hysa
