# controleDeLancamentos
Controle de lançamentos (débitos e créditos) e saldo diário consolidado.

## Rodando com Docker Compose

1. Suba os containers:

```powershell
docker compose up --build
```

2. Acesse:
- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- pgAdmin: `http://localhost:5050`

> O arquivo `.env` na raiz já contém os valores para banco, pgAdmin e JWT usados pelo `docker-compose.yml`.

## Gerar JWT para testes

Com a API no ar, gere um token de teste:

```powershell
curl -X POST "http://localhost:8080/api/autenticacao/token-teste" ^
  -H "Content-Type: application/json" ^
  -d "{\"usuario\":\"leonardo\"}"
```

A resposta retorna `accessToken` (Bearer). Use esse token para chamar os endpoints protegidos:

```powershell
curl -X POST "http://localhost:8080/api/lancamentos" ^
  -H "Authorization: Bearer SEU_TOKEN" ^
  -H "Content-Type: application/json" ^
  -d "{\"valor\":150.75,\"tipo\":\"Credito\",\"dataLancamento\":\"2026-05-30T10:00:00\",\"descricao\":\"Recebimento\"}"
```

```powershell
curl "http://localhost:8080/api/consolidado-diario?dataReferencia=2026-05-30" ^
  -H "Authorization: Bearer SEU_TOKEN"
```
