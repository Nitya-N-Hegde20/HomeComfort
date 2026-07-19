import { Component, OnInit } from '@angular/core';
import { Category, Product } from '../../../shared/models/product.model';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-category-detail',
  imports: [CommonModule,RouterLink],
  templateUrl: './category-detail.html',
  styleUrl: './category-detail.css',
})
export class CategoryDetail implements OnInit {
  category: Category | null = null;
  products: Product[] = [];
  isLoading = true;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const categoryId = params['id'];
      this.loadCategory(categoryId);
    });
  }

  loadCategory(id: number) {
    this.apiService.getCategoryById(id).subscribe({
      next: (data) => {
        this.category = data;
        this.loadCategoryProducts(id);
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Category not found';
        this.isLoading = false;
      }
    });
  }

loadCategoryProducts(categoryId: number) {
  this.apiService.getProducts().subscribe({
    next: (allProducts) => {
      this.products = allProducts.filter(p => p.categoryId === Number(categoryId));
    }
  });
}
}