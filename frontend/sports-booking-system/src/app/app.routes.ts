import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LayoutComponent } from './layout/layout';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then(m => m.RegisterComponent)
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then(m => m.DashboardComponent)
      },
      {
        path: 'parks',
        loadComponent: () => import('./features/parks/parks-list/parks-list').then(m => m.ParksListComponent)
      },
      {
        path: 'parks/:id',
        loadComponent: () => import('./features/parks/park-detail/park-detail').then(m => m.ParkDetailComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./features/profile/profile').then(m => m.ProfileComponent)
      },
      {
        path: 'friends',
        loadComponent: () => import('./features/friends/friends').then(m => m.FriendsComponent)
      },
      {
        path: 'bookings/create/:fieldId',
        loadComponent: () => import('./features/bookings/create-booking/create-booking').then(m => m.CreateBookingComponent)
      },
      {
        path: 'palettes',
        loadComponent: () => import('./features/palettes-preview/palettes-preview').then(m => m.PalettesPreviewComponent)
      },
    ]
  },
  { path: '**', redirectTo: '/dashboard' }
];