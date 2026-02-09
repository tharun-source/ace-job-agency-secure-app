-- ====================================================================
-- Enable Two-Factor Authentication (2FA) for Your Account
-- ====================================================================
-- This script enables 2FA for a specific email address
-- Run this script using Visual Studio's SQL Server Object Explorer
-- ====================================================================

USE AceJobAgencyDB;
GO

-- Enable 2FA for your account
UPDATE Members 
SET IsTwoFactorEnabled = 1 
WHERE Email = 'tharunranjan38@gmail.com';
GO

-- Verify the change was successful
SELECT 
    Email,
    FirstName,
    LastName,
 IsTwoFactorEnabled,
    CurrentOtp,
    OtpExpiry,
    CreatedDate,
    LastLoginDate
FROM Members 
WHERE Email = 'tharunranjan38@gmail.com';
GO

-- Expected Result:
-- IsTwoFactorEnabled should be: 1 (True)
-- CurrentOtp should be: NULL (will be populated after login)
-- OtpExpiry should be: NULL (will be populated after login)

PRINT 'Two-Factor Authentication has been enabled successfully!';
PRINT 'You can now test 2FA by logging in with your credentials.';
GO
