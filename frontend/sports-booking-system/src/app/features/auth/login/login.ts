import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';


@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink, CardModule, InputTextModule, PasswordModule, ButtonModule, MessageModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  error = signal<string | null>(null);

  form = new FormGroup({
    email: new FormControl('',[Validators.required,Validators.email]),
    password: new FormControl('',[Validators.required])
  });

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
}
