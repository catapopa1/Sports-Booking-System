import { AfterViewInit, Component, ElementRef, NgZone, inject, signal, viewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { environment } from '../../../../environments/environment';


@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink, InputTextModule, PasswordModule, ButtonModule, MessageModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginComponent implements AfterViewInit {
  private auth = inject(AuthService);
  private router = inject(Router);
  private zone = inject(NgZone);

  readonly googleClientId = environment.googleClientId;
  readonly googleBtn = viewChild<ElementRef<HTMLDivElement>>('googleBtn');

  loading = signal(false);
  error = signal<string | null>(null);

  form = new FormGroup({
    email: new FormControl('',[Validators.required,Validators.email]),
    password: new FormControl('',[Validators.required])
  });

  ngAfterViewInit(): void {
    this.tryRenderGoogleButton();
  }

  private tryRenderGoogleButton(attempt = 0): void {
    if (!this.googleClientId) return;
    const host = this.googleBtn()?.nativeElement;
    if (!host) return;

    if (typeof google === 'undefined' || !google.accounts?.id) {
      if (attempt < 30) {
        setTimeout(() => this.tryRenderGoogleButton(attempt + 1), 100);
      }
      return;
    }

    google.accounts.id.initialize({
      client_id: this.googleClientId,
      callback: resp => this.zone.run(() => this.handleGoogleCredential(resp.credential)),
    });

    google.accounts.id.renderButton(host, {
      theme: 'outline',
      size: 'large',
      text: 'continue_with',
      shape: 'pill',
      width: 320,
    });
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid)
      return;

    this.loading.set(true);
    this.error.set(null);

    try {
      await this.auth.login({
        email: this.form.value.email!,
        password: this.form.value.password!
      });
      this.router.navigate(['/parks']);
    } catch {
    this.error.set('Invalid Email or Password');
    } finally {
    this.loading.set(false);
    }
  }

  private async handleGoogleCredential(idToken: string): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      await this.auth.googleLogin(idToken);
      this.router.navigate(['/parks']);
    } catch {
      this.error.set('Google sign-in failed. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }
}
