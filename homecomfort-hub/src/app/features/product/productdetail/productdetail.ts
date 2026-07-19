import { Component, OnInit } from '@angular/core';
import { Product } from '../../../shared/models/product.model';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-productdetail',
  imports: [CommonModule],
  templateUrl: './productdetail.html',
  styleUrl: './productdetail.css',
})
export class Productdetail implements OnInit {
  product: Product | null = null;
  isLoading = true;
  error: string | null = null;
  selectedImage = 0;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const productId = params['id'];
      this.loadProduct(productId);
    });
  }

  loadProduct(id: number) {
    this.apiService.getProductById(id).subscribe({
      next: (data) => {
        this.product = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Product not found';
        this.isLoading = false;
      }
    });
  }

  selectImage(index: number) {
    this.selectedImage = index;
  }

  addToCart() {
    alert(`${this.product?.name} added to cart!`);
  }

}
