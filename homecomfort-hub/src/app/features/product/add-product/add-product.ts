import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { Category } from '../../../shared/models/product.model';

@Component({
  selector: 'app-add-product',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './add-product.html',
  styleUrl: './add-product.css',
})
export class AddProduct implements OnInit {
  categories: Category[] = [];

productForm = new FormGroup({
  name: new FormControl('', Validators.required),
  price: new FormControl(0, [Validators.required, Validators.min(0)]),
  image: new FormControl(''),
  description: new FormControl(''),
  categoryId: new FormControl(0, Validators.required),
  amazonLink: new FormControl(''),
  flipkartLink: new FormControl(''),
  rating: new FormControl(0),
  reviewSummary: new FormControl('')
});

  constructor(private apiService: ApiService, private router: Router) {}

  ngOnInit() {
    this.apiService.getCategories().subscribe({
      next: (data) => this.categories = data,
      error: (err) => console.error('Error loading categories', err)
    });
  }

  onSubmit() {
    if (this.productForm.valid) {
      this.apiService.createProduct(this.productForm.value as any).subscribe({
        next: () => this.router.navigate(['/products']),
        error: (err) => console.error('Error creating product', err)
      });
    }
  }
}