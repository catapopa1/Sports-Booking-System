import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  readonly darkMode = signal<boolean>(false);

  init(): void {
    const isDark = localStorage.getItem('theme') === 'dark';
    this.darkMode.set(isDark);
    document.documentElement.classList.toggle('dark', isDark);
  }

  toggle(): void {
    const next = !this.darkMode();
    this.darkMode.set(next);
    document.documentElement.classList.toggle('dark', next);
    localStorage.setItem('theme', next ? 'dark' : 'light');
  }
}
