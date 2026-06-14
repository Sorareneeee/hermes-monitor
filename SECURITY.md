# Security Policy

## Reporting a Vulnerability

We take the security of Hermes Monitor seriously. If you discover a security vulnerability, please **do not** open a public issue. Instead, send a private report to the repository owner via GitHub's private vulnerability reporting feature:

1. Go to https://github.com/Sorareneeee/hermes-monitor/security/advisories
2. Click **"Report a vulnerability"**
3. Provide a detailed description of the issue

We will respond within 48 hours and work to address the issue promptly.

## Scope

Hermes Monitor is a desktop application that runs locally on your machine. It:

- Scans running processes (read-only)
- Reads JSON config files (read-only)
- Does **not** send any data over the network
- Does **not** store or transmit credentials

## Best Practices

- Always download Hermes Monitor from the official GitHub releases
- Verify the executable checksum matches the release notes
- Review the source code if you have security concerns
