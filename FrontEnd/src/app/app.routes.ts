import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { HomeAssistantConnection } from './pages/home-assistant-connection/home-assistant-connection';
import { PowerEntitySetup } from './pages/power-entity-setup/power-entity-setup';

export const routes: Routes = [
	{
		path: '',
		component: Home,
	},
	{
		path: 'home-assistant-connection',
		component: HomeAssistantConnection,
	},
	{
		path: 'power-entity-setup',
		component: PowerEntitySetup,
	},
];
