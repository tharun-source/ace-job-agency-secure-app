-- ====================================================================
-- Get Current OTP Code from Database
-- ====================================================================
-- Use this to retrieve the OTP if you can't see it in the console
-- ====================================================================

USE AceJobAgencyDB;
GO

-- Get the current OTP code
SELECT 
    Email,
    CurrentOtp AS 'OTP_CODE',
    OtpExpiry AS 'Expires_At',
    CASE 
        WHEN OtpExpiry > GETDATE() THEN 'VALID'
        ELSE 'EXPIRED'
  END AS 'Status',
    DATEDIFF(SECOND, GETDATE(), OtpExpiry) AS 'Seconds_Remaining'
FROM Members 
WHERE Email = 'tharunranjan38@gmail.com';
GO

PRINT '======================================';
PRINT 'Copy the OTP_CODE value and enter it on the verification page';
PRINT 'You have 5 minutes (300 seconds) from when it was generated';
PRINT '======================================';
GO
