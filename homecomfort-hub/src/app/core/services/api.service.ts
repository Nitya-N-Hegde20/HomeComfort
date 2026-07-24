import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { BlogPost, Category, Product } from '../../shared/models/product.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
 private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Products
  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/products`);
  }

searchProducts(term: string): Observable<Product[]> {
  return this.http.get<Product[]>(
    `${this.apiUrl}/products/search?term=${encodeURIComponent(term)}`
  );
}

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/products/${id}`);
  }

  // Categories
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/categories`);
  }

  getCategoryById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/categories/${id}`);
  }

  // Blog
  getBlogPosts(): Observable<BlogPost[]> {
    return this.http.get<BlogPost[]>(`${this.apiUrl}/blog`);
  }

  getBlogPostById(id: number): Observable<BlogPost> {
    return this.http.get<BlogPost>(`${this.apiUrl}/blog/${id}`);
  } 

  // Products
createProduct(product: Omit<Product, 'id'>): Observable<Product> {
  return this.http.post<Product>(`${this.apiUrl}/products`, product);
}

// Categories
createCategory(category: Omit<Category, 'id' | 'productCount'>): Observable<Category> {
  return this.http.post<Category>(`${this.apiUrl}/categories`, category);
}
}
