import { Component, OnInit } from '@angular/core';
import { Product } from '../../../shared/models/product.model';
import { ApiService } from '../../../core/services/api.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TruncatePipe } from '../../../shared/components/TruncatePipe';

@Component({
  selector: 'app-productlist',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TruncatePipe],
  templateUrl: './productlist.html',
  styleUrl: './productlist.css',
})
export class Productlist implements OnInit {
  allProducts: Product[] = [];
  filteredProducts: Product[] = [];

  isLoading = true;
  error: string | null = null;

  searchTerm = '';
  selectedCategory = '';
  minPrice = 0;
  maxPrice = 100000;

  currentPage = 1;
  itemsPerPage = 9;
  totalPages = 1;

  categories: string[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.apiService.getProducts().subscribe({
      next: (data) => {
        this.allProducts = data;
        this.extractCategories();
        this.applyFilters();
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load products';
        this.isLoading = false;
      }
    });
  }

  extractCategories() {
    const names = this.allProducts
      .map(p => p.category?.name)
      .filter((name): name is string => !!name);
    this.categories = [...new Set(names)];
  }

  applyFilters() {
    this.filteredProducts = this.allProducts.filter(product => {
      const matchesSearch = product.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
                           product.description.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesCategory = !this.selectedCategory || product.category?.name === this.selectedCategory;
      const matchesPrice = product.price >= this.minPrice && product.price <= this.maxPrice;

      return matchesSearch && matchesCategory && matchesPrice;
    });

    this.currentPage = 1;
    this.calculatePagination();
  }

  calculatePagination() {
    this.totalPages = Math.ceil(this.filteredProducts.length / this.itemsPerPage);
  }

  get paginatedProducts(): Product[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.filteredProducts.slice(startIndex, endIndex);
  }

  onSearchChange() {
    this.applyFilters();
  }

  onCategoryChange() {
    this.applyFilters();
  }

  onPriceChange() {
    this.applyFilters();
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  clearFilters() {
    this.searchTerm = '';
    this.selectedCategory = '';
    this.minPrice = 0;
    this.maxPrice = 100000;
    this.applyFilters();
  }
}