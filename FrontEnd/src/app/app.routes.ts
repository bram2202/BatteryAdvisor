import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { SetupWizard } from './pages/setup-wizard/setup-wizard';

export const routes: Routes = [
	{
		path: '',
		component: Home,
	},
	{
		path: 'setup-wizard',
		component: SetupWizard,
	},
];
