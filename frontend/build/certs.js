import * as fs from 'fs';
import * as child_process from 'child_process';
import * as path from 'path';

// this module takes the ASP.NET Core IIS certificate and uses it for Vite's dev server.
// this allows us to use HTTPS in development without having to generate a new certificate.

export function getAspDotNetCertificate() {
	// where the certificate and key is going to live
	const baseFolder =
		process.env.APPDATA !== undefined && process.env.APPDATA !== ''
			? `${process.env.APPDATA}/ASP.NET/https`
			: `${process.env.HOME}/.aspnet/https`;

	// get the cert name from the NPM package name.
	const certificateName = process.env.npm_package_name;

	if (!certificateName) {
		console.error('Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<> explicitly.')
		process.exit(-1);
	}

	const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
	const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

	// check if the cert and key already exist
	if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
		// export a new copy of the cert and key from .NET
		const fetchCert = child_process.spawnSync('dotnet', [
			'dev-certs',
			'https',
			'--export-path',
			certFilePath,
			'--format',
			'Pem',
			'--no-password',
		], { stdio: 'inherit', });

		const exitCode = fetchCert.status ?? 0;
		if (exitCode !== 0) {
			process.exit(exitCode)
		}
	}

	// read the cert and key as UTF8 strings
	const certificate = fs.readFileSync(certFilePath, 'utf8');
	const privateKey = fs.readFileSync(keyFilePath, 'utf8');

	// export the cert and key so we can use in them our dev server configuration
	return {
		certificate,
		privateKey
	}
}
