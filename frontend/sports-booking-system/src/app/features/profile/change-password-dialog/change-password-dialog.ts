import { Component, inject, model, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';
import { UsersService } from '../../../core/services/users.service';

@Component({
  selector: 'app-change-password-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    DialogModule,
    ButtonModule,
    PasswordModule,
    MessageModule,
  ],
  templateUrl: './change-password-dialog.html',
  styleUrl: './change-password-dialog.scss',
})
export class ChangePasswordDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly users = inject(UsersService);
  private readonly toast = inject(MessageService);

  readonly visible = model<boolean>(false);
  readonly submitting = signal<boolean>(false);
  readonly mismatch = signal<boolean>(false);

  readonly form = this.fb.nonNullable.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]],
  });

  async submit(): Promise<void> {
    this.mismatch.set(false);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const { currentPassword, newPassword, confirmPassword } = this.form.getRawValue();
    if (newPassword !== confirmPassword) {
      this.mismatch.set(true);
      return;
    }

    this.submitting.set(true);
    try {
      await this.users.changePassword(currentPassword, newPassword);
      this.toast.add({ severity: 'success', summary: 'Password updated' });
      this.visible.set(false);
    } finally {
      this.submitting.set(false);
    }
  }

  cancel(): void {
    this.visible.set(false);
  }

  onHide(): void {
    this.form.reset();
    this.mismatch.set(false);
    this.submitting.set(false);
  }
}
