// types/product.ts
export interface Product {
  id: number;
  article?: string;
  title: string;
  price: number;
  quantity: number;
  description?: string;
  photo?: string;
  status: string;
  idBrand?: number;
  brand?: string;
  idCategory?: number;
  category?: string;
}