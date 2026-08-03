# Code Signing Configuration

## Overview

AsyncAwaitBestPractices supports Windows Authenticode code signing to ensure compatibility with Windows 11 Smart App Control. This feature helps prevent the security warnings that occur when Smart App Control is enabled.

## How It Works

### Build Process
- Code signing is automatically enabled during Release builds when running on Windows
- Signing occurs after the build completes for each target framework
- The signing process is conditional and won't break builds when certificates are not available

### Requirements
- Valid Windows code signing certificate (`.pfx` file)
- `signtool.exe` available in PATH (included with Windows SDK)
- Windows build environment (signing is skipped on other platforms)

## Configuration

### Environment Variables
Set these environment variables to enable code signing:

```bash
WINDOWS_CODESIGN_CERTIFICATE=path/to/your/certificate.pfx
WINDOWS_CODESIGN_PASSWORD=your-certificate-password
```

### MSBuild Properties
Alternatively, you can set these MSBuild properties:

```xml
<PropertyGroup>
  <CodeSigningCertificatePath>path/to/your/certificate.pfx</CodeSigningCertificatePath>
  <CodeSigningCertificatePassword>your-certificate-password</CodeSigningCertificatePassword>
  <CodeSigningTimestampServer>http://timestamp.digicert.com</CodeSigningTimestampServer>
</PropertyGroup>
```

## Azure DevOps Pipeline

The Azure DevOps pipeline is configured to automatically sign assemblies when:
1. A secure file named by `WINDOWS_CODESIGN_CERTIFICATE_NAME` variable is available
2. The `WINDOWS_CODESIGN_PASSWORD` variable is set
3. The build is running on Windows

### Pipeline Setup
1. Upload your code signing certificate as a secure file in Azure DevOps
2. Set the `WINDOWS_CODESIGN_CERTIFICATE_NAME` variable to the name of your secure file
3. Set the `WINDOWS_CODESIGN_PASSWORD` variable as a secret variable

## Certificate Requirements

### For Windows 11 Smart App Control
- Certificate must be issued by a trusted Certificate Authority
- Certificate must be valid for code signing
- Certificate should have timestamping enabled for long-term validity

### Recommended Certificate Authorities
- DigiCert
- Sectigo (formerly Comodo)
- GlobalSign
- Entrust

## Troubleshooting

### Common Issues
- **Certificate not found**: Ensure the certificate path is correct and the file exists
- **Invalid password**: Verify the certificate password is correct
- **Timestamp server unavailable**: The build will retry timestamping, or you can change the timestamp server URL
- **signtool not found**: Install Windows SDK or ensure signtool.exe is in your PATH

### Local Development
Code signing is disabled by default for local development. To test signing locally:
1. Obtain a code signing certificate
2. Set the required environment variables
3. Build in Release configuration on Windows

## Security Considerations

- **Never commit certificates to source control**
- **Use secure storage for certificates and passwords**
- **Regularly update certificates before expiration**
- **Use separate certificates for different environments if needed**

## Verification

### Checking if an Assembly is Signed
You can verify if an assembly is signed using:

```powershell
# PowerShell
Get-AuthenticodeSignature "AsyncAwaitBestPractices.dll"

# Command Prompt
signtool verify /pa "AsyncAwaitBestPractices.dll"
```

### Properties Dialog
Right-click the DLL file and select "Properties" → "Digital Signatures" tab to view signature information.