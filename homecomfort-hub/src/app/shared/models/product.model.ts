export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  image: string;
  categoryId: number;
  category?: Category;   
  rating: number;
  reviewSummary?: string;
  amazonLink?: string;
  flipkartLink?: string;
}

export interface Category {
  id: number;
  name: string;
  description: string;
  image: string;
  productCount: number;
}

export interface BlogPost {
  id: number;
  title: string;
  content: string;
  author: string;
  date: string;
  image: string;
  category: string;
  readTime: number;
}

export interface Review {
  id: number;
  productId: number;
  rating: number;
  comment: string;
  author: string;
  date: string;
}