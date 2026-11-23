import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Produto, ProdutoBody } from '../../../app.interface';

export interface ProdutoCriadoResponse {
  id: number;
  mensagem: string;
}

@Injectable({
  providedIn: 'root'
})

export class GerenciarProdutosService {
  private apiUrl = `${environment.apiUrl}`; // Substitua pela URL correta da API do seu backend

  constructor(
    private http: HttpClient
  ) {}

  cadastrarProduto(dados: Omit<ProdutoBody, 'imagens'>): Observable<ProdutoCriadoResponse> {
    return this.http.post<ProdutoCriadoResponse>(`${this.apiUrl}/Produtos`, dados);
  }

  uploadImagem(produtoId: number, arquivo: File, referencia: number): Observable<unknown> {
    const formData = new FormData();
    formData.append('file', arquivo);
    formData.append('referencia', String(referencia));

    return this.http.post(`${this.apiUrl}/Produtos/${produtoId}/imagens`, formData);
  }

  getProduto(id: number) : Observable<Produto>{
    return this.http.get<Produto>(`${this.apiUrl}/Produtos/${id}`);
  }

  editarProduto(id: number, body: ProdutoBody) : Observable<unknown>{
    return this.http.put(`${this.apiUrl}/Produtos/${id}`, body);
  }
}


/*
Copyright Google LLC. All Rights Reserved.
Use of this source code is governed by an MIT-style license that
can be found in the LICENSE file at https://angular.io/license
*/
