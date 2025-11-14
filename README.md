# MyValidator

MyValidator is a .NET library designed to simplify and enhance data validation in your applications. It provides a robust and extensible framework for validating user input, ensuring data integrity, and reducing boilerplate code.
# MyValidator

Repositório contendo a solução `MyValidator` e o pacote `Mert1s.MyValidator` — uma biblioteca de validação leve e extensível para .NET.

Resumo rápido
--
- O pacote principal está em `Src/Mert1s.MyValidator` e fornece builders fluentes para declarar regras de validação (`ValidatorBuilder<T>`, `RuleBuilder`, `CollectionRuleBuilder`).
- Alvo: `net9.0` (veja `Src/Mert1s.MyValidator/Mert1s.MyValidator.csproj`).

Leia a documentação específica do pacote
--
O README detalhado do pacote foi adicionado em `Src/Mert1s.MyValidator/README.md`. Ele traz exemplos de uso, links de build/test e explicações sobre a API. Para abrir o README do pacote, confira:

- `Src/Mert1s.MyValidator/README.md`

Build e testes
--
Para compilar a solução:

```powershell
dotnet build Mert1s.MyValidator.sln -c Release
```

Para executar os testes de unidade:

```powershell
dotnet test Tests\UnitTests\UnitTests.csproj
```

Contribuindo
--
- Abra issues para bugs ou sugestões.
- Envie PRs pequenos e focados; inclua testes sempre que possível.

Licença
--
Este repositório está sob licença MIT (veja o arquivo `LICENSE` na raiz, se presente).

Contato
--
Abra uma issue no GitHub: https://github.com/AndersonCosta28/MyValidator

---

Se quiser, eu posso também:
- Atualizar o `README.md` raiz com exemplos de uso rápidos extraídos do `Src`.
- Adicionar um `CHANGELOG.md` básico com histórico de versões.

Diga qual opção prefere.