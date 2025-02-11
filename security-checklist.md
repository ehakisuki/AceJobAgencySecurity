# Web Application Security Checklist

## Registration and User Data Management
- [1 ] Implement successful saving of member info into the database 
- [ 1] Check for duplicate email addresses and handle appropriately
- [ 1] Implement strong password requirements:
  - [ 1] Minimum 12 characters
  - [1 ] Combination of lowercase, uppercase, numbers, and special characters
  - [ 1] Provide feedback on password strength
  - [ 1] Implement both client-side and server-side password checks
- [ error so removed ] Encrypt sensitive user data in the database (e.g., NRIC, credit card numbers)
- [ 1 ] Implement proper password hashing and storage
- [ 1] Implement file upload restrictions (e.g., .docx, .pdf, or .jpg only)

## Session Management
- [1 ] Create a secure session upon successful login
- [1 ] Implement session timeout
- [1 ] Route to homepage/login page after session timeout
- [? ] Detect and handle multiple logins from different devices/browser tabs

## Login/Logout Security
- [ 1] Implement proper login functionality
- [ 1] Implement rate limiting (e.g., account lockout after 3 failed login attempts)
- [ 1] Perform proper and safe logout (clear session and redirect to login page)
- [ 1] Implement audit logging (save user activities in the database)
- [ 1] Redirect to homepage after successful login, displaying user info

## Anti-Bot Protection
- [ code is there but cannot work] Implement Google reCAPTCHA v3 service

## Input Validation and Sanitization
- [ 1] Prevent injection attacks (e.g., SQL injection)
- [ 1] Implement Cross-Site Request Forgery (CSRF) protection
- [ 1] Prevent Cross-Site Scripting (XSS) attacks
- [ 1] Perform proper input sanitization, validation, and verification for all user inputs
- [1 ] Implement both client-side and server-side input validation
- [ 1] Display error or warning messages for improper input
- [ 1] Perform proper encoding before saving data into the database

## Error Handling
- [1 ] Implement graceful error handling on all pages
- [ 1] Create and display custom error pages (e.g., 404, 403)

## Software Testing and Security Analysis
- [ 1] Perform source code analysis using external tools (e.g., GitHub)
- [ ] Address security vulnerabilities identified in the source code

## Advanced Security Features
- [1 ] Implement automatic account recovery after lockout period
- [ no] Enforce password history (avoid password reuse, max 2 password history)
- [ ] Implement change password functionality
- [ ] Implement reset password functionality (using email link or SMS)
- [1 ] Enforce minimum and maximum password age policies
- [ ] Implement Two-Factor Authentication (2FA)

## General Security Best Practices
- [1 ] Use HTTPS for all communications
- [ 1] Implement proper access controls and authorization
- [1 ] Keep all software and dependencies up to date
- [ 1] Follow secure coding practices
- [1 ] Regularly backup and securely store user data
- [1 ] Implement logging and monitoring for security events



## Documentation and Reporting
- [ ] Prepare a report on implemented security features
- [ ] Complete and submit the security checklist

Remember to test each security feature thoroughly and ensure they work as expected in your web application.
