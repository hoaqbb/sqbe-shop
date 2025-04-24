import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { AccountService } from '../../../core/services/account.service';
import { Router, RouterLink } from '@angular/router';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { environment } from '../../../../environments/environment.development';
import { ToastrService } from 'ngx-toastr';
declare var google: any;

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CalendarModule, TextInputComponent, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit{
  registerForm: FormGroup;
  validationErrors: string[] = [];
  maxYear: Date;
  googleClientId = environment.googleClientId;

  constructor(
    private accountService: AccountService,
    private formBuilder: FormBuilder,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.maxYear = new Date();
    this.maxYear.setFullYear(this.maxYear.getFullYear() - 16);
    this.initializeGoogleBtn();
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      lastName: ['', [Validators.required, Validators.maxLength(20)]],
      firstName: ['', [Validators.required, Validators.maxLength(20)]],
      gender: ['0', Validators.required],
      dateOfBirth: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });
    this.registerForm.controls?.['password'].valueChanges.subscribe(() => {
      this.registerForm.controls?.['confirmPassword'].updateValueAndValidity();
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null
        : { isMatching: true };
    };
  }

  register() {
    this.validationErrors = [];
    if(this.registerForm.invalid) return this.validationErrors.push("Vui lòng nhập thông tin hợp lệ!");
    
    return this.accountService.register(this.registerForm.value).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: (error) => {
        console.log(error);
        (this.validationErrors.push(error.error))
      },
    });
  }

  initializeGoogleBtn() {
    google.accounts.id.initialize({
      client_id: this.googleClientId,
      callback: (res: any) => {
        this.accountService.signInWithGoogle(res.credential).subscribe({
          next: () => this.router.navigateByUrl('/'),
          error: (err) => {
            this.toastr.error(err.error);
          }
        });
      },
    });

    google.accounts.id.renderButton(document.getElementById('google-btn'), {
      size: 'large',
      shape: 'square',
      text: 'signup_with',
    });
  }
}
