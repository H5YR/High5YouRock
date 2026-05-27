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
		console.log('🔒 Setting up HTTPS certificate for Vite dev server...');
		
		// check if ASP.NET Core dev certificate exists and is trusted
		const checkCert = child_process.spawnSync('dotnet', [
			'dev-certs',
			'https',
			'--check',
			'--trust'
		], { stdio: 'pipe' });

		// if cert doesn't exist or isn't trusted, create and trust it
		if (checkCert.status !== 0) {
			console.log('📝 Creating and trusting ASP.NET Core development certificate...');
			console.log('   (You may be prompted to allow certificate installation)');
			
			const trustCert = child_process.spawnSync('dotnet', [
				'dev-certs',
				'https',
				'--trust'
			], { stdio: 'inherit' });

			if (trustCert.status !== 0) {
				console.error('❌ Failed to create/trust the development certificate.');
				console.error('   Please run manually: dotnet dev-certs https --trust');
				process.exit(trustCert.status ?? -1);
			}
			
			console.log('✅ Certificate created and trusted successfully!');
		}

		// ensure the directory exists before exporting the certificate
		if (!fs.existsSync(baseFolder)) {
			console.log(`📁 Creating certificate directory: ${baseFolder}`);
			fs.mkdirSync(baseFolder, { recursive: true });
		}

		// export a new copy of the cert and key from .NET
		console.log('📤 Exporting certificate for Vite...');
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
			console.error('❌ Failed to export certificate.');
			process.exit(exitCode)
		}
		
		console.log('✅ Certificate setup complete!');
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
