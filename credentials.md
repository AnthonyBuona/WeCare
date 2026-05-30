# 🔐 Credenciais de Acesso e Personas - WeCare

O banco de dados foi resetado e populado com novos dados de teste. Abaixo estão os acessos disponíveis e as personas configuradas.

**Senha padrão para todos os usuários:** `1q2w3E*`

---

## 🏢 1. Ambiente Host (Painel Administrativo Técnico)
O Host é utilizado exclusivamente pela equipe técnica do WeCare para gerenciamento de acessos, tenants e monitoramento do sistema. **Não contém dados de negócio.**

| Usuário | E-mail | Cargo / Persona |
| :--- | :--- | :--- |
| **Super Admin** | `admin@abp.io` | Acesso total, gerenciamento de tenants e infraestrutura. |
| **Suporte Técnico** | `suporte@wecare.com` | Equipe técnica responsável por suporte e manutenção de acessos. |

---

## 🏥 2. Ambiente Tenant: Clínica Bem Viver
Este é o ambiente onde o negócio acontece. Todos os dados de pacientes e tratamentos estão isolados aqui.

| Usuário | E-mail | Persona Detalhada |
| :--- | :--- | :--- |
| **Admin Tenant** | `admin@abp.io` | Administrador da unidade Clínica Bem Viver. |
| **Terapeuta** | `terapeuta@7d7f722f.com` | **Dra. Camila**: Especialista em Terapia Ocupacional. |
| **Responsável** | `responsavel@7d7f722f.com` | **Maria Responsável**: Mãe do paciente Lucas. |

> *Nota: No ABP, o login do admin pode ser o mesmo e-mail, mas o sistema diferencia o acesso baseado no domínio ou header de tenant selecionado.*

---

## 🧑‍⚕️ Cenário de Teste Configurado (Tenant)

Para facilitar seus testes dentro da **Clínica Bem Viver**, o seguinte cenário foi montado:

- **Paciente:** Lucas Paciente (8 anos).
- **Tratamento:** Psicopedagogia.
- **Objetivo Atual:** "Desenvolvimento Motor" (Iniciado há 1 mês).
- **Treinamento:** "Coordenação Fina" (Exercícios para fortalecer a coordenação motora fina).
- **Consulta Recente:** Sessão realizada ontem pela Dra. Camila com a descrição "Sessão inicial produtiva".

---

## 🚀 Como testar?
1. Acesse [https://localhost:44373/](https://localhost:44373/).
2. Para logar no **Host**, use o admin sem especificar tenant.
3. Para logar na **Clínica Bem Viver**, mude o tenant no link "Change" na tela de login para `ClinicaBemViver`.
