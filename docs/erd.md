# ERD

В модели 11 доменных сущностей: `Citizen`, `Address`, `AuthorityOffice`, `Employee`, `Summons`, `SummonsStatusHistory`, `Notification`, `DeliveryAttempt`, `Appeal`, `Document`, `AuditEvent`.

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
      string MiddleName
      date BirthDate
      string Email
      string Phone
    }
    ADDRESS {
      uuid Id PK
      uuid CitizenId FK
      string PostalCode
      string Region
      string City
      string Street
      string Building
      string Apartment
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
      string Comment
    }
    SUMMONS_STATUS_HISTORY {
      uuid Id PK
      uuid SummonsId FK
      string FromStatus
      string ToStatus
      datetime ChangedAt
      string ChangedBy
      string Comment
    }
    NOTIFICATION {
      uuid Id PK
      uuid SummonsId FK
      string Channel
      string DestinationMasked
      string Status
      datetime CreatedAt
    }
    DELIVERY_ATTEMPT {
      uuid Id PK
      uuid NotificationId FK
      int AttemptNumber
      string Result
      datetime AttemptedAt
      string ProviderMessage
    }
    APPEAL {
      uuid Id PK
      uuid SummonsId FK
      string Type
      string Text
      string Status
      datetime SubmittedAt
    }
    DOCUMENT {
      uuid Id PK
      uuid SummonsId FK
      string FileName
      string MimeType
      string StorageUri
      datetime CreatedAt
    }
    AUDIT_EVENT {
      uuid Id PK
      uuid SummonsId FK
      string Action
      string Actor
      datetime OccurredAt
      string Details
    }
```
