import { Component, OnInit } from '@angular/core';
import { Category } from '../../../shared/models/product.model';
import { ApiService } from '../../../core/services/api.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-category-grid',
  imports: [CommonModule,RouterLink],
  templateUrl: './category-grid.html',
  styleUrl: './category-grid.css',
})
export class CategoryGrid  implements OnInit {
  categories: Category[] = [];
  isLoading = true;
  error: string | null = null;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.apiService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load categories';
        this.isLoading = false;
      }
    });
  }
}
