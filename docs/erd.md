# ERD

В модели 12 сущностей, включая отдельную системную учётную запись для UC-11.

```mermaid
erDiagram
    CITIZEN ||--|| ADDRESS : has
    CITIZEN ||--o{ SUMMONS : receives
    AUTHORITY_OFFICE ||--o{ EMPLOYEE : employs
    AUTHORITY_OFFICE ||--o{ SUMMONS : issues
    EMPLOYEE ||--o{ SUMMONS : creates
    SUMMONS ||--o{ SUMMONS_STATUS_HISTORY : has
    SUMMONS ||--o{ NOTIFICATION : triggers
    NOTIFICATION ||--o{ DELIVERY_ATTEMPT : has
    SUMMONS ||--o{ APPEAL : may_have
    SUMMONS ||--o{ DOCUMENT : has
    SUMMONS ||--o{ AUDIT_EVENT : audited_by

    CITIZEN {
      uuid Id PK
      string RegistryNumber UK
      string LastName
      string FirstName
      date BirthDate
      string Email
      string Phone
    }
    ADDRESS {
      uuid Id PK
      uuid CitizenId FK
      string Region
      string City
      string Street
      string Building
    }
    AUTHORITY_OFFICE {
      uuid Id PK
      string Code UK
      string Name
      string Region
    }
    EMPLOYEE {
      uuid Id PK
      uuid AuthorityOfficeId FK
      string PersonnelNumber UK
      string FullName
      string Role
    }
    SUMMONS {
      uuid Id PK
      string Number UK
      uuid CitizenId FK
      uuid AuthorityOfficeId FK
      uuid CreatedByEmployeeId FK
      date IssuedAt
      datetime DueAt
      string Reason
      string Status
    }
    SUMMONS_STATUS_HISTORY {
      uuid Id PK
      uuid SummonsId FK
      string FromStatus
      string ToStatus
      datetime ChangedAt
      string ChangedBy
    }
    NOTIFICATION {
      uuid Id PK
      uuid SummonsId FK
      string Channel
      string Status
    }
    DELIVERY_ATTEMPT {
      uuid Id PK
      uuid NotificationId FK
      int AttemptNumber
      string Result
    }
    APPEAL {
      uuid Id PK
      uuid SummonsId FK
      string Type
      string Text
      string Status
    }
    DOCUMENT {
      uuid Id PK
      uuid SummonsId FK
      string FileName
      string MimeType
      string StorageUri
    }
    AUDIT_EVENT {
      uuid Id PK
      uuid SummonsId FK
      string Action
      string Actor
    }
    SYSTEM_ACCOUNT {
      uuid Id PK
      string ExternalSubject UK
      string DisplayName
      string Role
      string CitizenRegistryNumber
      bool IsActive
    }
```

`Citizen` в коде соответствует карточке призывника, а `AuthorityOffice` — военкомату. Эти технические имена сохранены, чтобы не ломать существующую initial migration.
