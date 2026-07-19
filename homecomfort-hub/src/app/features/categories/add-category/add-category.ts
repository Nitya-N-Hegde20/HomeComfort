import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-category',
  imports: [ReactiveFormsModule],
  templateUrl: './add-category.html',
  styleUrl: './add-category.css',
})
export class AddCategory {
categoryForm = new FormGroup({
    name: new FormControl('', Validators.required),
    description: new FormControl(''),
    image: new FormControl('')
  });

  constructor(private apiService: ApiService, private router: Router) {}

  onSubmit() {
    if (this.categoryForm.valid) {
      this.apiService.createCategory(this.categoryForm.value as any).subscribe({
        next: () => this.router.navigate(['/categories']),
        error: (err) => console.error('Error creating category', err)
      });
    }
  }
}
