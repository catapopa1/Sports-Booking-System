import { Component , inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink, CardModule, InputTextModule, PasswordModule, ButtonModule, MessageModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class RegisterComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  error = signal<string | null>(null);

  form = new FormGroup({
    firstName: new FormControl('',[Validators.required, Validators.maxLength(100)]),
    lastName: new FormControl('', [Validators.required, Validators.maxLength(100)]),
    email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(256)]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

  async onSubmit(): Promise<void> {
    if (this.form.invalid) 
      return;
    
    this.loading.set(true);
    this.error.set(null);
    
    try {
      await this.auth.register({
        firstName: this.form.value.firstName!,
        lastName: this.form.value.lastName!,
        email: this.form.value.email!,
        password: this.form.value.password!
      });
      this.router.navigate(['/login']);
    } catch {
      this.error.set('Registration failed. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }

  
}
